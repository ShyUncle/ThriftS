/**
 * Autogenerated by Thrift Compiler (0.9.2)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
package org.zeeman.thrifts.idl;


public enum ThriftSCompression implements org.apache.thrift.TEnum {
  None(0),
  Gzip(1);

  private final int value;

  private ThriftSCompression(int value) {
    this.value = value;
  }

  /**
   * Get the integer value of this enum value, as defined in the Thrift IDL.
   */
  public int getValue() {
    return value;
  }

  /**
   * Find a the enum type by its integer value, as defined in the Thrift IDL.
   * @return null if the value is not found.
   */
  public static ThriftSCompression findByValue(int value) { 
    switch (value) {
      case 0:
        return None;
      case 1:
        return Gzip;
      default:
        return null;
    }
  }
}